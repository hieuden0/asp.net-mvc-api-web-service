﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleTester.service1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="AppWebService", ConfigurationName="service1.Service1")]
    public interface Service1 {
        
        [System.ServiceModel.OperationContractAttribute(Action="AppWebService/Service1/DoWork", ReplyAction="AppWebService/Service1/DoWorkResponse")]
        string DoWork();
        
        [System.ServiceModel.OperationContractAttribute(Action="AppWebService/Service1/DoWork", ReplyAction="AppWebService/Service1/DoWorkResponse")]
        System.Threading.Tasks.Task<string> DoWorkAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="AppWebService/Service1/Login", ReplyAction="AppWebService/Service1/LoginResponse")]
        string Login(string UserName, string Password, bool RememberMe);
        
        [System.ServiceModel.OperationContractAttribute(Action="AppWebService/Service1/Login", ReplyAction="AppWebService/Service1/LoginResponse")]
        System.Threading.Tasks.Task<string> LoginAsync(string UserName, string Password, bool RememberMe);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Service1Channel : ConsoleTester.service1.Service1, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Service1Client : System.ServiceModel.ClientBase<ConsoleTester.service1.Service1>, ConsoleTester.service1.Service1 {
        
        public Service1Client() {
        }
        
        public Service1Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Service1Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string DoWork() {
            return base.Channel.DoWork();
        }
        
        public System.Threading.Tasks.Task<string> DoWorkAsync() {
            return base.Channel.DoWorkAsync();
        }
        
        public string Login(string UserName, string Password, bool RememberMe) {
            return base.Channel.Login(UserName, Password, RememberMe);
        }
        
        public System.Threading.Tasks.Task<string> LoginAsync(string UserName, string Password, bool RememberMe) {
            return base.Channel.LoginAsync(UserName, Password, RememberMe);
        }
    }
}
