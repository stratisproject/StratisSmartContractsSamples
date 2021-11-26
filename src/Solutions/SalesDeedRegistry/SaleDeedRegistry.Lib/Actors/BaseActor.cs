namespace SaleDeedRegistry.Lib.Actors
{
    public class BaseActor
    {
        /// <summary>
        /// Dump the object info on console
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="name">Name</param>
        protected virtual void Dump(object obj, string name = "Receipt Response")
        {
            using (var writer = new System.IO.StringWriter())
            {
                ObjectDumper.Dumper.Dump(obj, name, writer);
                System.Console.Write(writer.ToString());
            };
        }
    }
}
