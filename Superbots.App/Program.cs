using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Superbots.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*
             * ELECTRON INFO
             * 
             * ATTENZIONE:
             * Electron ha alcuni comandi fondamentali.
             * 1) electronize init 
             *    posizionandosi al livello del progetto, in questo caso:
             *    
             *    -> Superbots(SolutionName) 
             *      -> Superbots.App(ProjectName)
             *      
             *    Con questo comando verrà creato principalmente il file: electron.manifest.json
             *    nel quale sono contenute le informazioni per nodejs e non solo per creare una build in electron.
             *    
             * 2) Con il comando init in precendenza teoricamente è stato aggiunto in VisualStudio un profilo di Debug
             *    per Electron, quindi se si vuole debuggare è possible cambiare il profilo di default "https" in "Electron.NET App"
             *    
             * 3) electronize build /target win
             *    questo comando creerà una build per windows (win), linux (linux), iOS (osx)
             *    ATTENZIONE perché verrano create delle cartelle con nomi simile, ma contenuto incompleto!
             *    Data la seguente struttura:
             *    
             *    -> Superbots(SolutionName)
             *      -> bin*
             *          -> Desktop
             *              -> AppInstaller.exe
             *      -> obj
             *      -> Superbots.App(ProjectName)
             *          -> bin
             *          -> obj
             *          
             *    Il contenuto delle cartelle bin e obj anche se in livelli diversi è molto simile.
             *    La build completa del progetto la si troverà nella cartella bin*.
             *    Queste cartelle sono usante anche da VisualStudio non solo da Electron, ma dovrebbero solo
             *    contenere i passaggi intermedi per avere una build finale, per questo il loro contenuto è incompleto.
             *    
             *    Per esempio in questo progetto viene utilizzato SQLite, il quale crea un database su un file data.db
             *    il file di database non è incluso nelle altre cartelle (bin e obj) e di conseguenza AppInstaller.exe 
             *    che troveremo non inculuderà il file di database. L'unica cartella con AppInstaller.exe contente tutti i file extra
             *    si troverà, come detto prima, in bin*
             *      
             */
            //Electron to run like an native OS App
            builder.WebHost.UseElectron(args);
            builder.Services.AddElectron();

            var app = builder.AddServices().Build();

            app.ConfigureApp();

            if (HybridSupport.IsElectronActive)
            {
                var options = new BrowserWindowOptions();
                options.Width = 1500;
                options.Height = 1200;
                options.Icon = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\img\\AiIcon.png");
                var window = await Electron.WindowManager.CreateWindowAsync(options);
                //window.WebContents.OpenDevTools();
            }

            await app.RunAsync();
        }
    }
}