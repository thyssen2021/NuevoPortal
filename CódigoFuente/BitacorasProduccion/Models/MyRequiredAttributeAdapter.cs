using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Models
{
    public class MyRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public MyRequiredAttributeAdapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute) : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                attribute.ErrorMessageResourceName = "Required";
                attribute.ErrorMessageResourceType = typeof(Resources.Mensajes);
            }
           
        }
    }

    public class MyMaxLengthAttributeAdapter : MaxLengthAttributeAdapter
    {
        public MyMaxLengthAttributeAdapter(ModelMetadata metadata, ControllerContext context, MaxLengthAttribute attribute) : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                attribute.ErrorMessageResourceName = "MaxLength";
                attribute.ErrorMessageResourceType = typeof(Resources.Mensajes);
            }                        
        }
    }

    public class MyMinLengthAttributeAdapter : MinLengthAttributeAdapter
    {
        public MyMinLengthAttributeAdapter(ModelMetadata metadata, ControllerContext context, MinLengthAttribute attribute) : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                attribute.ErrorMessageResourceName = "MinLength";
                attribute.ErrorMessageResourceType = typeof(Resources.Mensajes);
            }           
        }
    }

    public class MyRangeAttributeAdapter : RangeAttributeAdapter
    {
        public MyRangeAttributeAdapter(ModelMetadata metadata, ControllerContext context, RangeAttribute attribute) : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                attribute.ErrorMessageResourceName = "Range";
                attribute.ErrorMessageResourceType = typeof(Resources.Mensajes);
            }
           
        }
    }

    public class MyRegularExpressionAttributeAdapter : RegularExpressionAttributeAdapter
    {
        public MyRegularExpressionAttributeAdapter(ModelMetadata metadata, ControllerContext context, RegularExpressionAttribute attribute) : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                attribute.ErrorMessageResourceName = "ExpresionRegular";
                attribute.ErrorMessageResourceType = typeof(Resources.Mensajes);
            }
            
        }
    }

    public class MyStringLengthAttributeAdapter : StringLengthAttributeAdapter
    {
        public MyStringLengthAttributeAdapter(ModelMetadata metadata, ControllerContext context, StringLengthAttribute attribute) : base(metadata, context, attribute)
        {
            if (string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                attribute.ErrorMessageResourceName = "StringLength";
                attribute.ErrorMessageResourceType = typeof(Resources.Mensajes);
            }                       
        }
    }
}