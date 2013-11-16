using System.Configuration;
using RMArt.Core;

namespace RMArt.Web
{
	public class ConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("SiteTitle", DefaultValue = "Gallery")]
		public string SiteTitle
		{
			get { return (string)this["SiteTitle"]; }
			set { this["SiteTitle"] = value; }
		}

		[ConfigurationProperty("PicturesDirectory", DefaultValue = "~/App_Data/Pictures/")]
		public string PicturesDirectory
		{
			get { return (string)this["PicturesDirectory"]; }
			set { this["PicturesDirectory"] = value; }
		}

		[ConfigurationProperty("ThumbsDirectory", DefaultValue = "~/App_Data/Thumbs/")]
		public string ThumbsDirectory
		{
			get { return (string)this["ThumbsDirectory"]; }
			set { this["ThumbsDirectory"] = value; }
		}

		[ConfigurationProperty("MaxFileSize", DefaultValue = null)]
		public long? MaxFileSize
		{
			get { return (long?)this["MaxFileSize"]; }
			set { this["MaxFileSize"] = value; }
		}

		[ConfigurationProperty("DefaultCulture", DefaultValue = "en")]
		public string DefaultCulture
		{
			get { return (string)this["DefaultCulture"]; }
			set { this["DefaultCulture"] = value; }
		}

		[ConfigurationProperty("AdditionalCultures")]
		public CulturesCollection AdditionalCultures
		{
			get { return (CulturesCollection)this["AdditionalCultures"]; }
			set { this["AdditionalCultures"] = value; }
		}

		[ConfigurationProperty("AutodetectCulture", DefaultValue = true)]
		public bool AutodetectCulture
		{
			get { return (bool)this["AutodetectCulture"]; }
			set { this["AutodetectCulture"] = value; }
		}

		[ConfigurationProperty("DefaultPicturesSortOrder", DefaultValue = PicturesSortOrder.Newest)]
		public PicturesSortOrder DefaultPicturesSortOrder
		{
			get { return (PicturesSortOrder)this["DefaultPicturesSortOrder"]; }
			set { this["DefaultPicturesSortOrder"] = value; }
		}

		[ConfigurationProperty("DefaultTagsSortOrder", DefaultValue = TagsSortOrder.Newest)]
		public TagsSortOrder DefaultTagsSortOrder
		{
			get { return (TagsSortOrder)this["DefaultTagsSortOrder"]; }
			set { this["DefaultTagsSortOrder"] = value; }
		}

		[ConfigurationProperty("ThumbnailSize", DefaultValue = 0)]
		public int ThumbnailSize
		{
			get { return (int)this["ThumbnailSize"]; }
			set { this["ThumbnailSize"] = value; }
		}

		[ConfigurationProperty("TooltipThumbnailSize", DefaultValue = 1)]
		public int TooltipThumbnailSize
		{
			get { return (int)this["TooltipThumbnailSize"]; }
			set { this["TooltipThumbnailSize"] = value; }
		}

		[ConfigurationProperty("ThmbnailsOnPageCount", DefaultValue = 30)]
		[IntegerValidator(MinValue = 1)]
		public int ThmbnailsOnPageCount
		{
			get { return (int)this["ThmbnailsOnPageCount"]; }
			set { this["ThmbnailsOnPageCount"] = value; }
		}

		[ConfigurationProperty("UploadsPremoderation", DefaultValue = true)]
		public bool UploadsPremoderation
		{
			get { return (bool)this["UploadsPremoderation"]; }
			set { this["UploadsPremoderation"] = value; }
		}

		[ConfigurationProperty("DefaultRating", DefaultValue = Rating.Unrated)]
		public Rating DefaultRating
		{
			get { return (Rating)this["DefaultRating"]; }
			set { this["DefaultRating"] = value; }
		}

		[ConfigurationProperty("DefaultMaxRating", DefaultValue = Rating.SFW)]
		public Rating DefaultMaxRating
		{
			get { return (Rating)this["DefaultMaxRating"]; }
			set { this["DefaultMaxRating"] = value; }
		}

		[ConfigurationProperty("DefaultShowUnrated", DefaultValue = false)]
		public bool DefaultShowUnrated
		{
			get { return (bool)this["DefaultShowUnrated"]; }
			set { this["DefaultShowUnrated"] = value; }
		}

		[ConfigurationProperty("EmailConfirmationEnabled", DefaultValue = true)]
		public bool EmailConfirmationEnabled
		{
			get { return (bool)this["EmailConfirmationEnabled"]; }
			set { this["EmailConfirmationEnabled"] = value; }
		}

		[ConfigurationProperty("ReCapthchaPublicKey", DefaultValue = "")]
		public string ReCapthchaPublicKey
		{
			get { return (string)this["ReCapthchaPublicKey"]; }
			set { this["ReCapthchaPublicKey"] = value; }
		}

		[ConfigurationProperty("ReCapthchaPrivateKey", DefaultValue = "")]
		public string ReCapthchaPrivateKey
		{
			get { return (string)this["ReCapthchaPrivateKey"]; }
			set { this["ReCapthchaPrivateKey"] = value; }
		}
	}
}