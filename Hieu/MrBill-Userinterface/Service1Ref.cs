﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MrBill_MVC.Controllers
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Test", Namespace="http://schemas.datacontract.org/2004/07/MrBill_MVC.Controllers")]
    public partial class Test : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string Namek__BackingFieldField;
        
        private string testk__BackingFieldField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<Name>k__BackingField", IsRequired=true)]
        public string Namek__BackingField
        {
            get
            {
                return this.Namek__BackingFieldField;
            }
            set
            {
                this.Namek__BackingFieldField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="<test>k__BackingField", IsRequired=true)]
        public string testk__BackingField
        {
            get
            {
                return this.testk__BackingFieldField;
            }
            set
            {
                this.testk__BackingFieldField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="test", ConfigurationName="Service1")]
public interface Service1
{
    
    [System.ServiceModel.OperationContractAttribute(Action="test/Service1/DoWork", ReplyAction="test/Service1/DoWorkResponse")]
    MrBill_MVC.Controllers.Test DoWork();
    
    [System.ServiceModel.OperationContractAttribute(Action="test/Service1/DoWork", ReplyAction="test/Service1/DoWorkResponse")]
    System.Threading.Tasks.Task<MrBill_MVC.Controllers.Test> DoWorkAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="test/Service1/GetData", ReplyAction="test/Service1/GetDataResponse")]
    string GetData(int value);
    
    [System.ServiceModel.OperationContractAttribute(Action="test/Service1/GetData", ReplyAction="test/Service1/GetDataResponse")]
    System.Threading.Tasks.Task<string> GetDataAsync(int value);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface Service1Channel : Service1, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class Service1Client : System.ServiceModel.ClientBase<Service1>, Service1
{
    
    public Service1Client()
    {
    }
    
    public Service1Client(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public Service1Client(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public Service1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public MrBill_MVC.Controllers.Test DoWork()
    {
        return base.Channel.DoWork();
    }
    
    public System.Threading.Tasks.Task<MrBill_MVC.Controllers.Test> DoWorkAsync()
    {
        return base.Channel.DoWorkAsync();
    }
    
    public string GetData(int value)
    {
        return base.Channel.GetData(value);
    }
    
    public System.Threading.Tasks.Task<string> GetDataAsync(int value)
    {
        return base.Channel.GetDataAsync(value);
    }
}
