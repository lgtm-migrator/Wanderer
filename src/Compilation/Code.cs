﻿using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using NLua;
using Wanderer.Systems;

namespace Wanderer.Compilation
{
    public abstract class Code 
    {
        public string Script { get; set; }

        
        [JsonConstructor]
        public Code()
        {
        }

        public Code(string script)
        {
            Script = script;
        }


        protected ScriptOptions GetScriptOptions()
        {
            return ScriptOptions.Default
                .WithReferences(typeof(ICondition<>).Assembly)
                .WithImports(
                    "Wanderer.Stats", 
                    "System",
                    "System.Linq",
                    "Wanderer",
                    "Wanderer.Actors",
                    "Wanderer.Actions",
                    "Wanderer.Systems",
                    "Wanderer.Adjectives");
        }

        public Lua GetLua(object o)
        {
            var lua = GetLua();

            foreach(var prop in o.GetType().GetProperties())
            {
                var val = prop.GetValue(o);

                if(val != null)
                    lua[prop.Name] = val;
            }

            return lua;
        }

        protected Lua GetLua()
        {
            var lua =  new Lua();

                lua.LoadCLRPackage();
                lua.DoString(@"
import ('Wanderer', 'Wanderer')
import ('Wanderer','Wanderer.Stats')
import ('Wanderer','Wanderer.Places')
import ('Wanderer','Wanderer.Actions')
import ('System','System')
import ('Wanderer','Wanderer.Systems')
import ('Wanderer','Wanderer.Adjectives')

");
            return lua;
        }

        public void Run(SystemArgs a)
        {
            using(var lua = GetLua(a))
            {
                lua.DoString(Script);
            }
        }
        
        public override string ToString()
        {
            return Script ;
        }
        public static void ApplyGuidConstructorFix(Lua lua)
        {
            //TODO: this is a hacky workaround for guid constructor seemingly returning null
            lua.DoString("GuidClass=luanet.import_type('System.Guid')");
            lua.DoString("Guid=luanet.get_constructor_bysig(GuidClass,'System.String')");
        }
    }
}