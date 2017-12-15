===============================================================================
                              N o e s i s G U I
===============================================================================

  Company: Noesis Technologies
  Web: http://www.noesisengine.com
  Contact: info@noesisengine.com

===============================================================================


Documentation and Changelog
---------------------------

The documentation is installed in the folder NoesisDoc/ next to Assets/ the
first time the package is imported.

  Assets/
  NoesisDoc/
    index.html

The Changelog is located at NoesisDoc/Doc/Gui.Core.Changelog.html


Package Folder Structure
------------------------

NoesisGUI package files are organized as follows:

  Assets/
    NoesisGUI/
      Samples/               Sample scenes ready to try and useful as start point
      Themes/                Sample theme files to skin your user interfaces
      UserControls/          Reusable UserControls used in samples
      README.txt             This file
    
    Editor/
      NoesisGUI/             Editor extension scripts for XAML asset processing
        BuildTool/           Libraries used by editor extension scripts
  
    Plugins/
      NoesisGUI/
        Scripts/
          Core/              Runtime plugin scripts
          MVVM/              Model View ViewModel helper classes
          Proxies/           Wrapper classes to communicate with NoesisGUI
          NoesisGUIPanel.cs  The GUI script component
      Android/               Runtime library for Android
      iOS/                   Runtime static library for iOS
      Metro/                 Runtime library for Windows Phone and Windows Store
      x86/                   Runtime library for Windows & Linux 32 bits
      x86_64/                Runtime library for Windows 64 bits
      NoesisUnityRenderHook  Library that hooks to Unity native rendering

    StreamingAssets/
      NoesisGUI/             Processed data for each supported platform

  NoesisDoc/                 Documentation and tutorials
    index.html               Index of the documentation

  Temp/                      Folder where logs are stored
  Dumps/                     Folder where crash dumps are stored


Set up instructions
-------------------

After importing NoesisGUI package to your project, we recommend opening the
documentation index, through the main menu Tools -> NoesisGUI -> Documentation.

You will find a list of tutorials about NoesisGUI, please read them all
carefully, with special attention Unity Integration tutorial. It contains step
by step instructions to use NoesisGUI to provide an awesome user interface for
your Unity project.

You will notice that after package was imported, NoesisGUI is ready to be used.
Just select any sample scene under NoesisGUI/Samples and hit the play button.


Online support
--------------

If you have any questions or suffer any inconvenient, please visit our forums
at http://forums.noesisengine.com.
