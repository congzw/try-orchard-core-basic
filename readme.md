# try orchard core basic features

## 1 notes

### 1.1 指定发布后的运行端口

appsettings.json
{
	...
    "Urls": "http://*:9300;https://*:9301;"
}

### 1.2 启用Razor运行时编译支持

- 1 add nuget package: Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- 2 services.AddControllersWithViews().AddRazorRuntimeCompilation(); (参考1.3)
- 3 edit mainApp and module proj target (参考1.4)
- 4 如果发现： Cannot find reference assembly 'Microsoft.AspNetCore.Antiforgery.dll' file for package Microsoft.AspNetCore.Antiforgery，可以尝试（不是必然）：    
<PreserveCompilationReferences>true</PreserveCompilationReferences>
<PreserveCompilationContext>true</PreserveCompilationContext>
- 5 貌似在项目文件中增加<PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="3.1.18" />，步骤4中的问题不在出现
- 6 some hack for developing and publishing(参考1.4)
- 7 至此，部署后，动态替换.cshtml, js, css将得到支持。

### 1.3 OrchardCore的运行时Razor编译的支持

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

### 1.4 同时支持开发和发布阶段的一些Hack

- ViewFiles(Views,Pages)
- ContentFiles(wwwroot)
- 为了实现想要的模块复制逻辑，采用一些文件夹路径和字符替换的Hack操作

根据自定义发布标志IsPublish，在[嵌入资源类型]和[内容类型]这二者间进行转换，这样可以实现：

- 开发阶段使用: 嵌入资源类型 EmbeddedResource
- 发布阶段使用: 内容类型 Content

主应用：

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
	
模块：

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

发布脚本:

call dotnet restore -r win-x64
call dotnet publish -p:IsPublish=true -c Release -r win-x64 --self-contained false --no-restore --output bin/publish/ModularApplication/win-x64

pause