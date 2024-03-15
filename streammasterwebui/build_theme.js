// Importing necessary modules
import { exec } from 'child_process';
import { promises as fs } from 'fs';

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

// Function to compile SCSS and copy CSS
async function compileAndCopySass(sassInput, sassOutput, destinationPath) {
  try {
    // Compile SCSS to CSS
    await execPromise(`sass --update ${sassInput}:${sassOutput}`);

    // Copy the CSS file
    await fs.copyFile(sassOutput, destinationPath);
    console.log(`CSS file successfully copied to ${destinationPath}`);
  } catch (error) {
    console.error('Failed to compile and copy CSS:', error);
  }
}

// Define paths for dark and light themes
const paths = [
  {
    sassInput: 'themes/streammaster/streammaster-dark/theme.scss',
    sassOutput: 'themes/streammaster/streammaster-dark.css',
    destinationPath: 'lib/styles/streammaster-dark.css'
  },
  {
    sassInput: 'themes/streammaster/streammaster-light/theme.scss',
    sassOutput: 'themes/streammaster/streammaster-light.css',
    destinationPath: 'lib/styles/streammaster-light.css'
  }
];

// Run the function for both themes
paths.forEach(({ sassInput, sassOutput, destinationPath }) => {
  compileAndCopySass(sassInput, sassOutput, destinationPath);
});
