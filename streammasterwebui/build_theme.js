import { exec } from 'child_process';
import { promises as fs } from 'fs';
import path from 'path';

// Helper function to run shell commands
const execPromise = (command) =>
  new Promise((resolve, reject) => {
    exec(command, (error, stdout, stderr) => {
      if (error) {
        reject(error);
      } else {
        resolve({ stdout, stderr });
      }
    });
  });

// Function to ensure destination directory exists
async function ensureDirExists(dirPath) {
  await fs.mkdir(dirPath, { recursive: true });
}

// Function to compile SCSS and copy CSS
async function compileAndCopySass(sassInput, sassOutput, destinationPaths) {
  try {
    // Compile SCSS to CSS
    await execPromise(`sass --update ${sassInput}:${sassOutput}`);

    // Ensure each destination directory exists and copy the CSS file
    await Promise.all(
      destinationPaths.map(async (destinationPath) => {
        const dirPath = path.dirname(destinationPath);
        await ensureDirExists(dirPath);
        await fs.copyFile(sassOutput, destinationPath);
        console.log(`CSS file successfully copied to ${destinationPath}`);
      })
    );
  } catch (error) {
    console.error('Failed to compile and copy CSS:', error);
  }
}

// Function to copy the contents of a directory
async function copyDirectoryContents(srcDir, destDir) {
  try {
    const files = await fs.readdir(srcDir, { withFileTypes: true });
    await ensureDirExists(destDir); // Ensure the destination directory exists

    await Promise.all(
      files.map(async (file) => {
        const srcPath = path.join(srcDir, file.name);
        const destPath = path.join(destDir, file.name);

        if (file.isDirectory()) {
          await copyDirectoryContents(srcPath, destPath);
        } else {
          await fs.copyFile(srcPath, destPath);
          console.log(`File ${file.name} successfully copied to ${destPath}`);
        }
      })
    );
  } catch (error) {
    console.error('Failed to copy directory contents:', error);
  }
}

// Function to copy a file from srcPath to destPath
async function copyFile(srcPath, destPath) {
  try {
    const dirPath = path.dirname(destPath);
    await ensureDirExists(dirPath); // Ensure the destination directory exists
    await fs.copyFile(srcPath, destPath); // Copy the file
    console.log(`File successfully copied from ${srcPath} to ${destPath}`);
  } catch (error) {
    console.error(`Failed to copy file from ${srcPath} to ${destPath}:`, error);
  }
}

// Example usage:
const paths = [
  {
    sassInput: 'themes/streammaster/streammaster-dark/theme.scss',
    sassOutput: 'themes/streammaster/streammaster-dark.css',
    destinationPaths: [
      'lib/styles/streammaster-dark.css',
      'public/themes/streammaster-dark/theme.css' // Second destination
    ]
  },
  {
    sassInput: 'themes/streammaster/streammaster-light/theme.scss',
    sassOutput: 'themes/streammaster/streammaster-light.css',
    destinationPaths: [
      'lib/styles/streammaster-light.css',
      'public/themes/streammaster-light/theme.css' // Second destination
    ]
  }
];

// Compile and copy CSS
paths.forEach(({ sassInput, sassOutput, destinationPaths }) => {
  compileAndCopySass(sassInput, sassOutput, destinationPaths);
});

// Copy font directory
const FontSrc = 'themes/streammaster/streammaster-base/fonts';
const darkFontDest = 'public/themes/streammaster-dark/fonts';
copyDirectoryContents(FontSrc, darkFontDest);

const ligthFontDest = 'public/themes/streammaster-light/fonts';
copyDirectoryContents(FontSrc, ligthFontDest);

let wwwrootFontDest = '..\\StreamMaster.API\\bin\\Debug\\net8.0\\wwwroot\\themes\\streammaster-light\\fonts';
copyDirectoryContents(FontSrc, wwwrootFontDest);
wwwrootFontDest = '..\\StreamMaster.API\\bin\\Debug\\net8.0\\wwwroot\\themes\\streammaster-dark\\fonts';
copyDirectoryContents(FontSrc, wwwrootFontDest);

let darkThemeSrc = 'lib/styles/streammaster-dark.css';
let darkThemeDest = 'public/themes/streammaster-dark/theme.css';
copyFile(darkThemeSrc, darkThemeDest);

let lightThemeSrc = 'lib/styles/streammaster-light.css';
let lightThemeDest = 'public/themes/streammaster-light/theme.css';
copyFile(lightThemeSrc, lightThemeDest);

darkThemeSrc = 'lib/styles/streammaster-dark.css';
darkThemeDest = '..\\StreamMaster.API\\bin\\Debug\\net8.0\\wwwroot\\themes\\streammaster-light\\theme.css';
copyFile(darkThemeSrc, darkThemeDest);

lightThemeSrc = 'lib/styles/streammaster-light.css';
lightThemeDest = '..\\StreamMaster.API\\bin\\Debug\\net8.0\\wwwroot\\themes\\streammaster-dark\\theme.css';
copyFile(lightThemeSrc, lightThemeDest);
