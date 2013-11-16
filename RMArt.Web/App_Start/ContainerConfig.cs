using Autofac;
using Autofac.Integration.Mvc;
using RMArt.Core;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace RMArt.Web
{
	public static class ContainerConfig
	{
		public static void InitContainer(HttpServerUtility server)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => new DataContextProvider("mainDB")).As<IDataContextProvider>().SingleInstance();

			builder.RegisterType<PicturesRepository>().As<IPicturesRepository>().SingleInstance();
			var picturesDirectoryPath = server.ToPhysicalPath(Config.Default.PicturesDirectory);
			builder.Register(c => new FileSystemRepository(picturesDirectoryPath)).Named<IFileRepository>("PicturesFileRepository").SingleInstance();
			var thumbsDirectoryPath = server.ToPhysicalPath(Config.Default.ThumbsDirectory);
			builder.Register(c => new FileSystemRepository(thumbsDirectoryPath)).Named<IFileRepository>("ThumbsFileRepository").SingleInstance();
			builder.RegisterType<DatabaseRatesRepository>().As<IRatesRepository>().SingleInstance();
			builder.RegisterType<DatabaseFavoritesRepository>().As<IFavoritesRepository>().SingleInstance();
			builder
				.Register(
					c => new PicturesService(
						c.Resolve<IPicturesRepository>(),
						c.ResolveNamed<IFileRepository>("PicturesFileRepository"),
						c.ResolveNamed<IFileRepository>("ThumbsFileRepository"),
						c.Resolve<IRatesRepository>(),
						c.Resolve<IFavoritesRepository>(),
						c.Resolve<ITagsService>(),
						c.Resolve<IHistoryService>(),
						c.Resolve<IUsersService>(),
						new Dictionary<int, Size> { { 0, new Size(170, 170) } },
						Config.Default.MaxFileSize))
				.As<IPicturesService>()
				.SingleInstance();

			builder.RegisterType<TagsRepository>().Named<ITagsRepository>("TagsRepository").SingleInstance();
			builder.RegisterDecorator<ITagsRepository>((c, inner) => new CachedTagsRepository(inner), "TagsRepository").SingleInstance();
			builder.RegisterType<TagsService>().As<ITagsService>().SingleInstance();

			builder.RegisterType<DatabaseUsersRepository>().As<IUsersRepository>().SingleInstance();
			builder.RegisterType<TicketsRepository>().As<ITicketsRepository>().SingleInstance();
			builder.RegisterType<UsersService>().As<IUsersService>().SingleInstance();
			builder.RegisterType<TicketsService>().As<ITicketsService>().SingleInstance();
			builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().SingleInstance();

			builder.RegisterType<DatabaseHistoryRepository>().As<IHistoryRepository>().SingleInstance();
			builder.RegisterType<HistoryService>().As<IHistoryService>().SingleInstance();

			builder.RegisterType<ReportsRepository>().As<IReportsRepository>().SingleInstance();
			builder.RegisterType<ReportsService>().As<IReportsService>().SingleInstance();

			builder.RegisterType<DatabaseContentRepository>().As<IContentRepository>().SingleInstance();

			builder.RegisterType<DiscussionService>().As<IDiscussionService>().SingleInstance();

			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
		}
	}
}