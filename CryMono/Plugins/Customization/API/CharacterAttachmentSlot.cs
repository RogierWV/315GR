﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using CryEngine;

namespace CryEngine.CharacterCustomization
{
    public class CharacterAttachmentSlot
    {
        internal CharacterAttachmentSlot(CustomizationManager manager, XElement element)
        {
			m_manager = manager;
            Element = element;

            Name = element.Attribute("name").Value;

            var allowNoneAttribute = element.Attribute("allowNone");
            if(allowNoneAttribute != null)
                CanBeEmpty = allowNoneAttribute.Value == "1";

            var hiddenAttribute = element.Attribute("hidden");
            if(hiddenAttribute != null)
                Hidden = hiddenAttribute.Value == "1";

			var subSlotElements = element.Elements("AttachmentSlot");
			if (subSlotElements.Count() > 0)
			{
				SubAttachmentSlots = new CharacterAttachmentSlot[subSlotElements.Count()];
				for(int i = 0; i < subSlotElements.Count(); i++)
				{
					var subSlotElement = subSlotElements.ElementAt(i);

					SubAttachmentSlots[i] = new CharacterAttachmentSlot(manager, subSlotElement);
				}
			}

			var mirroredSlotElements = element.Elements("MirroredSlot");
			if (mirroredSlotElements.Count() > 0)
			{
				MirroredSlots = new CharacterAttachmentSlot[mirroredSlotElements.Count()];
				for (int i = 0; i < mirroredSlotElements.Count(); i++)
				{
					var mirroredSlotElement = mirroredSlotElements.ElementAt(i);

					MirroredSlots[i] = new CharacterAttachmentSlot(manager, mirroredSlotElement);
				}
			}

			// Set up brands
			var slotBrandElements = element.Elements("Brand");

			int numBrands = slotBrandElements.Count();
			Brands = new CharacterAttachmentBrand[numBrands];

			for(int i = 0; i < numBrands; i++)
			{
				var brandElement = slotBrandElements.ElementAt(i);

				Brands[i] = new CharacterAttachmentBrand(this, brandElement);
			}
        }

        /// <summary>
        /// Clears the currently active attachment for this slot.
        /// </summary>
        public void Clear()
        {
			GetWriteableElement().SetAttributeValue("Binding", null);
        }

		public bool Write(CharacterAttachment attachment, XElement attachmentElement = null, bool overwrite = true)
		{
			if (attachment == null)
				throw new NullReferenceException("attachment");

			if (attachmentElement == null)
			{
				if (MirroredSlots != null && attachment.MirroredChildren != null)
				{
					foreach (var mirroredAttachment in attachment.MirroredChildren)
					{
						Write(attachment, GetWriteableElement(mirroredAttachment.Slot.Name));
						mirroredAttachment.Slot.Write(mirroredAttachment, null, false);
					}

					return true;
				}

				var currentAttachment = Current;
				if (currentAttachment != null)
				{
					if (currentAttachment.SubAttachmentVariations != null)
					{
						foreach (var subAttachment in currentAttachment.SubAttachmentVariations)
							subAttachment.Slot.Clear();
					}
				}

				attachmentElement = GetWriteableElement();
				if (attachmentElement == null)
					throw new CustomizationConfigurationException(string.Format("Failed to locate attachments for slot {0}!", Name));
			}

			if (overwrite || attachment.Name != null)
				attachmentElement.SetAttributeValue("Name", attachment.Name);

			if (overwrite || attachment.Type != null)
				attachmentElement.SetAttributeValue("Type", attachment.Type);
			if (overwrite || attachment.BoneName != null)
				attachmentElement.SetAttributeValue("BoneName", attachment.BoneName);

			if (overwrite || attachment.Object != null)
				attachmentElement.SetAttributeValue("Binding", attachment.Object);
			if (overwrite || attachment.Material != null)
			{
				var material = attachment.Material;
				string materialPath = null;

				if (material != null)
					materialPath = material.FilePath ?? material.BaseFilePath;

				attachmentElement.SetAttributeValue("Material", materialPath);
			}

			if (overwrite || attachment.Flags != null)
				attachmentElement.SetAttributeValue("Flags", attachment.Flags);

			if (overwrite || attachment.Position != null)
				attachmentElement.SetAttributeValue("Position", attachment.Position);
			if (overwrite || attachment.Rotation != null)
				attachmentElement.SetAttributeValue("Rotation", attachment.Rotation);

			if (attachment.SubAttachment != null)
				attachment.SubAttachment.Slot.Write(attachment.SubAttachment);

			return true;
		}

		public CharacterAttachmentBrand GetBrand(string name)
		{
			if (Brands != null)
			{
				foreach (var brand in Brands)
				{
					if (brand.Name == name)
						return brand;
				}
			}

			return null;
		}

        public CharacterAttachment GetAttachment(string name)
        {
			if (Brands != null)
			{
				foreach (var brand in Brands)
				{
					foreach (var attachment in brand.Attachments)
					{
						if (attachment.Name == name)
							return attachment;
					}
				}
			}

            return null;
        }

		XElement GetWriteableElement(string name = null)
		{
			var attachmentList = m_manager.CharacterDefinition.Element("CharacterDefinition").Element("AttachmentList");
			var attachmentElements = attachmentList.Elements("Attachment");

			if (name == null)
				name = Name;

			return attachmentElements.FirstOrDefault(x => x.Attribute("AName").Value == name);
		}

        public string Name { get; set; }

		/// <summary>
		/// Brands containing attachments.
		/// </summary>
		public CharacterAttachmentBrand[] Brands { get; set; }

        /// <summary>
        /// Gets the currently active attachment for this slot.
        /// </summary>
        public CharacterAttachment Current
        {
            get
            {
				var attachmentList = m_manager.CharacterDefinition.Element("CharacterDefinition").Element("AttachmentList");

                var attachmentElements = attachmentList.Elements("Attachment");
				var attachmentElement = attachmentElements.FirstOrDefault(x => x.Attribute("AName").Value == Name);
                if (attachmentElement == null)
                    throw new CustomizationConfigurationException(string.Format("Could not find slot element for {0}", Name));

				foreach (var brand in Brands)
				{
					foreach (var attachment in brand.Attachments)
					{
						var nameAttribute = attachmentElement.Attribute("Name");
						if (nameAttribute == null)
							continue;

						if (attachment.Name == nameAttribute.Value)
							return attachment;
					}
				}

                return null;
            }
        }

        /// <summary>
        /// Gets a random attachment for this slot.
        /// </summary>
        /// <returns>The randomed attachment, possibly null if <see cref="CanBeEmpty"/> is set to true.</returns>
        public CharacterAttachment RandomAttachment
        {
            get
            {
                var selector = new Random();

				var iRandom = selector.Next(Brands.Length);

				var brand = Brands[iRandom];

				iRandom = selector.Next(CanBeEmpty ? -1 : 0, brand.Attachments.Length);

                if (iRandom != -1)
					return brand.Attachments.ElementAt(iRandom);
                else
                    return null;
            }
        }

		public CharacterAttachmentSlot[] SubAttachmentSlots { get; set; }

		public CharacterAttachmentSlot[] MirroredSlots { get; set; }

        internal XElement Element { get; private set; }

        public bool CanBeEmpty { get; set; }

        public bool Hidden { get; set; }

		CustomizationManager m_manager;
    }
}
