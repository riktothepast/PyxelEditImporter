# Pyxel Edit Importer

A tool to import `.pyxel` files into `Unity Game Engine`.

### Features
---
- Automatic export of file layers into a single `.png`.
- Slices animation frames based on tile size.
- Creates animation clips for each one of the animations in the `.pyxel` file.

### Installation
-----

#### Unity package manager

Install the package `pyxelImporter` into your project, find the file `Packages/manifest.json` and edit it add this entry:

```json
{
  "dependencies": {
     "net.fiveotwo.pyxelImporter": "https://github.com/riktothepast/PyxelEditImporter.git#latest",
      ...
  },
}
```

#### The madperson way
- Download this project as a `.zip`
- Place the `Editor/PyxelEditImporter` folder somewhere in your project.

### Importing files
---- 
Just drag and drop new files into your project, the importer will detect and generate all files.
![File Import](https://github.com/riktothepast/PyxelEditImporter/blob/main/README_FileImport.gif)
Inside the folder `${yourFileName}_animations` you'll find all the clips created from the `.pyxel` file.
![Animations](https://github.com/riktothepast/PyxelEditImporter/blob/main/README_Animations.gif)

If for some reason you want to re import all files, you can go to `502 Tools/PyxelEditImporter/Re Import`

### Limitations
----
Right now, the following is not supported, this functionality may arrive as the project continues to evolve.
- Blending options in layers.
- Alpha values in layers.
- Tile imports.
- Slicing of frames that are not part of an animation.
