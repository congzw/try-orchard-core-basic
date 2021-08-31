# try orchard core basic features

## 1 notes

### 1.1 ָ������������ж˿�

appsettings.json
{
	...
    "Urls": "http://*:9300;https://*:9301;"
}

### 1.2 ����Razor����ʱ����֧��

- 1 add nuget package: Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- 2 services.AddControllersWithViews().AddRazorRuntimeCompilation(); (�ο�1.3)
- 3 edit mainApp and module proj target (�ο�1.4)
- 4 ������֣� Cannot find reference assembly 'Microsoft.AspNetCore.Antiforgery.dll' file for package Microsoft.AspNetCore.Antiforgery�����Գ��ԣ����Ǳ�Ȼ����    
<PreserveCompilationReferences>true</PreserveCompilationReferences>
<PreserveCompilationContext>true</PreserveCompilationContext>
- 5 ò������Ŀ�ļ�������<PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="3.1.18" />������4�е����ⲻ�ڳ���
- 6 some hack for developing and publishing(�ο�1.4)
- 7 ���ˣ�����󣬶�̬�滻.cshtml, js, css���õ�֧�֡�

### 1.3 OrchardCore������ʱRazor�����֧��

////OK => Default
//services
//    .AddOrchardCore()
//    .AddMvc();

////OK => access IMvcBuilder
//services
//    .AddOrchardCore()
//    .AddMvc()
//    .ConfigureServices(cfgServices =>
//    {
//        cfgServices.AddMvcCore().AddRazorRuntimeCompilation();
//    });

### 1.4 ͬʱ֧�ֿ����ͷ����׶ε�һЩHack

- ViewFiles(Views,Pages)
- ContentFiles(wwwroot)
- Ϊ��ʵ����Ҫ��ģ�鸴���߼�������һЩ�ļ���·�����ַ��滻��Hack����

�����Զ��巢����־IsPublish����[Ƕ����Դ����]��[��������]����߼����ת������������ʵ�֣�

- �����׶�ʹ��: Ƕ����Դ���� EmbeddedResource
- �����׶�ʹ��: �������� Content

��Ӧ�ã�

	<Target Name="CopyAreaViewsForPublish" AfterTargets="Publish">
		<Message Text="### PublishDir: '$(PublishDir)'" Importance="high" />
		<ItemGroup>
			<Views Include="..\*\Views\**" Exclude="..\ModularApplication\**" />
			<Views Include="..\*\Pages\**" Exclude="..\ModularApplication\**" />
		</ItemGroup>
		<Message Text="### Views: '@(Views)'" Importance="high" />
		<Copy SourceFiles="@(Views)" DestinationFolder="$(PublishDir)\Areas\HackFolder\%(Views.RelativeDir)" />
	</Target>

	<Target Name="CopyContentsForPublish" AfterTargets="Publish">
		<Message Text="### PublishDir: '$(PublishDir)'" Importance="high" />
		<ItemGroup>
			<ContentFiles Include="..\*\wwwroot\**" Exclude="..\ModularApplication\**" />
		</ItemGroup>
		<Message Text="### ContentFiles: '@(ContentFiles)'" Importance="high" />
		<Copy SourceFiles="@(ContentFiles)" DestinationFolder="$(PublishDir)\wwwroot\$([System.String]::Copy('%(ContentFiles.RecursiveDir)').Replace('wwwroot\', ''))" />
	</Target>
	
ģ�飺

	<Target Name="SwitchEmbeddedResourceToContentForPublish" BeforeTargets="BeforeBuild" Condition="'$(IsPublish)'=='true'">
		<Message Text="### MyBeforeBuild Module1 ### PublishProtocol: $(PublishProtocol)" Importance="high" />
		<ItemGroup>
			<EmbeddedResource Remove="Views\**\*.cshtml" />
			<EmbeddedResource Remove="Pages\**\*.cshtml" />
			<EmbeddedResource Remove="wwwroot\**\*.*" />
			<Content Include="Views\**\*.cshtml" CopyToPublishDirectory="Never" />
			<Content Include="Pages\**\*.cshtml" CopyToPublishDirectory="Never" />
			<Content Include="wwwroot\**\*.*" CopyToPublishDirectory="Never" />
		</ItemGroup>
	</Target>

�����ű�:

call dotnet restore -r win-x64
call dotnet publish -p:IsPublish=true -c Release -r win-x64 --self-contained false --no-restore --output bin/publish/ModularApplication/win-x64

pause