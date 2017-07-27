# FA-Sharp
A Windows 10 UWP App that parses FontAwesome Website and converts it to C# Classes for using the Fonts in C# applications, Xamarin

With being a Backer for Font Awesome 5, I started getting updates with the Font Files and Web Pages to demo the Fonts, I did not want to have to create new classes by hand for thousands of Fonts, so I made a parser. You give it the html document and it gives you the Classes with the FontFamily specified in each class, you can just add the Fonts to your Projects and go to town using Icons.


e.g. : fontawesome-5.0.0-alpha6-win\fontawesome-5.0.0-alpha6-win\docs\webfont.html - This would parse into C# Classes

Also http://fontawesome.io/cheatsheet/ .. There is a button for that.

This uses the HTMLAgilityPack for Parsing.
