using Engine;
using Engine.Graphics;
using Engine.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game
{
	public static class BlocksTexturesManager
	{
		public static List<string> m_blockTextureNames = [];

		public static Texture2D DefaultBlocksTexture
		{
			get;
			set;
		}

		public static ReadOnlyList<string> BlockTexturesNames => new(m_blockTextureNames);

		public static string BlockTexturesDirectoryName => ModsManager.BlockTexturesDirectoryName;

		public static event Action<string> BlocksTextureDeleted;

		public static void Initialize()
		{
			Storage.CreateDirectory(BlockTexturesDirectoryName);
			DefaultBlocksTexture = ContentManager.Get<Texture2D>("Textures/Blocks");
		}

		public static bool IsBuiltIn(string name)
		{
			return string.IsNullOrEmpty(name);
		}

		public static string GetFileName(string name)
		{
			if (IsBuiltIn(name))
			{
				return null;
			}
			return Storage.CombinePaths(BlockTexturesDirectoryName, name);
		}

		public static string GetDisplayName(string name)
		{
			if (IsBuiltIn(name))
			{
				return "Survivalcraft";
			}
			return Storage.GetFileNameWithoutExtension(name);
		}

		public static DateTime GetCreationDate(string name)
		{
			try
			{
				if (!IsBuiltIn(name))
				{
					return Storage.GetFileLastWriteTime(GetFileName(name));
				}
			}
			catch
			{
			}
			return new DateTime(2000, 1, 1);
		}

		public static Texture2D LoadTexture(string name)
		{
			Texture2D texture2D = null;
			if (!IsBuiltIn(name))
			{
				try
				{
					Image image = Image.Load(GetFileName(name));
					ValidateBlocksTexture(image);
					texture2D = Texture2D.Load(image);
					texture2D.Tag = image;
				}
				catch (Exception ex)
				{
					Log.Warning(string.Format("Could not load blocks texture \"{0}\". Reason: {1}.", new object[2] { name, ex.Message }));
				}
			}
			if (texture2D == null)
			{
				texture2D = DefaultBlocksTexture;
			}
			return texture2D;
		}

		public static string ImportBlocksTexture(string name, Stream stream)
		{
			Exception ex = ExternalContentManager.VerifyExternalContentName(name);
			if (ex != null)
			{
				throw ex;
			}
			if (Storage.GetExtension(name) != ".scbtex")
			{
				name += ".scbtex";
			}
			ValidateBlocksTexture(stream);
			stream.Position = 0L;
			using (Stream destination = Storage.OpenFile(GetFileName(name), OpenFileMode.Create))
			{
				stream.CopyTo(destination);
				return name;
			}
		}

		public static void DeleteBlocksTexture(string name)
		{
			try
			{
				string fileName = GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					Storage.DeleteFile(fileName);
					BlocksTextureDeleted?.Invoke(name);
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser($"Unable to delete blocks texture \"{name}\"", e);
			}
		}

		public static void UpdateBlocksTexturesList()
		{
			m_blockTextureNames.Clear();
			m_blockTextureNames.Add(string.Empty);
			foreach (string item in Storage.ListFileNames(BlockTexturesDirectoryName))
			{
				m_blockTextureNames.Add(item);
			}
		}

		public static void ValidateBlocksTexture(Stream stream)
		{
			var image = Image.Load(stream);
			if (image.Width > 65536 || image.Height > 65536)
			{
				throw new InvalidOperationException($"Blocks texture is larger than 65536x65536 pixels (size={image.Width}x{image.Height})");
			}
			if (!MathUtils.IsPowerOf2(image.Width) || !MathUtils.IsPowerOf2(image.Height))
			{
				throw new InvalidOperationException($"Blocks texture does not have power-of-two size (size={image.Width}x{image.Height})");
			}
		}

		public static void ValidateBlocksTexture(Image image)
		{
			if (image.Width > 65536 || image.Height > 65536)
			{
				throw new InvalidOperationException(string.Format("Blocks texture is larger than 65536x65536 pixels (size={0}x{1})", new object[2] { image.Width, image.Height }));
			}
			if (!MathUtils.IsPowerOf2(image.Width) || !MathUtils.IsPowerOf2(image.Height))
			{
				throw new InvalidOperationException(string.Format("Blocks texture does not have power-of-two size (size={0}x{1})", new object[2] { image.Width, image.Height }));
			}
		}
	}
}
