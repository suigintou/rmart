using Autofac;
using RMArt.Core;
using RMArt.ServiceTool.Properties;
using System.Collections.Generic;
using System.Linq;
using Thorn;

namespace RMArt.ServiceTool
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => new DataContextProvider("mainDB")).As<IDataContextProvider>().SingleInstance();

			builder.RegisterType<PicturesRepository>().As<IPicturesRepository>().SingleInstance();
			builder.Register(c => new FileSystemRepository(Settings.Default.PicturesDirectory)).Named<IFileRepository>("PicturesFileRepository").SingleInstance();
			builder.Register(c => new FileSystemRepository(Settings.Default.ThumbsDirectory)).Named<IFileRepository>("ThumbsFileRepository").SingleInstance();
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
						new Dictionary<int, Size> { { 0, new Size(170, 170) }, { 1, new Size(300, 300) } }))
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

			builder
				.RegisterAssemblyTypes(typeof(Program).Assembly)
				.Where(t => t.GetCustomAttributes(typeof(ThornExportAttribute), false).Any())
				.SingleInstance();

			var container = builder.Build();

			Runner
				.Configure(config => config.UseCallbackToInstantiateExports(container.Resolve))
				.Run(args);
		}
	}
}