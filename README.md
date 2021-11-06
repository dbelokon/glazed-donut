# Glazed Donut

Glazed Donut is a CLI Static Site Generator that generates a full static HTML website given a text file, or a directory that contains multiple text files.

## Features
* Supports CSS Stylesheets: allows you to style your website however you want, you just need to specify a URL to the CSS stylesheets.
* Destination Folder: you can choose a folder where the HTML files will be stored in. 
* Accepts a text file that will be converted to a static HTML page.
* Can convert multiple text files into HTML files at once if you provide a path to a folder with text files that you want to use.
* You can check program's version by typing -v or --version in the terminal.
* Displays a help message with the command usage instructions if you type -h or --help.
* Allows to input a language tag for the lang attribute in the generated HTML file(s).
* Supports horizontal rule for Markdown files.

## Usage

To convert your .txt files into .html files, the following commands should be used:

    -i <path to a text file or directory with text files>
    
      or
      
    --input <path to a text file or directory with text files>

### Other Commands That Can Be Utilized
You can also use the commands listed below when using this software:

|     Command    |                               Description                                   |
| -------------- | --------------------------------------------------------------------------- |
|-h, --help      |displays a list of supported commands and instructions on how to use them.   |
|-v, --version   |displays the name and the version of the software.                           |
|-s, --stylesheet|allows to specify a URL to a CSS stylesheet to style the generated HTML files.|
|-o, --output    |let's specify a destination folder for the generated HTML files. By default, they will be placed in the dist folder.|
|-l, --lang      |accepts a language tag to mark the HTML document. By default, the lang attribute will be set to en-CA|
|-c, --config    | indicates the path to a JSON config file.                                   |


## Example 

In this scenario, assume that you have a file called **"hello.txt"** and you want to convert it into **"hello.html"**.

Here is what the hello.txt looks like before:

    Hello! 
    
    Welcome to my static website!
   
    blah blah blah
    blah blah blah
    
    Bye!!
      
In the command line, type the following command: 

    -i <path to the hello.txt file>

or 

    --input <path to the hello.txt file>

This will create a dist folder, where the converted text file will be placed.

When you open dist/hello.html, you will see the following:

```html
    <!doctype html>
    <html lang=""en"">
    
    <head>
        <meta charset=""utf-8"">
        <title>Filename</title>
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    </head>
    
    <body>
        <p>Hello!</p>
    
        <p>Welcome to my static website!</p>
   
        <p>blah blah blah<br/>
        blah blah blah</p>
    
        <p>Bye!!</p>
    </body>
    
    </html>";
```

## Author

Diana Belokon ([@belokond](https://dev.to/belokond))


