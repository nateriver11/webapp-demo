using System;

namespace ASMWebTest1Project
{
    public interface IApplicationBuilder
    {
        void UseDeveloperExceptionPage();
        void UseStaticFiles();
        void UseMvc(Func<object, object> p);
    }
}