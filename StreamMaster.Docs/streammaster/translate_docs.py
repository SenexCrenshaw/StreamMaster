import os

from google.cloud import translate_v2 as translate

# Initialize the Google Cloud Translator
translate_client = translate.Client()

# Define source directory for English
source_dir = 'docs/en'

# Define target languages you want to support
languages = {
    'es': 'Spanish',
    'fr': 'French',
    'de': 'German',
    'pt': 'Portuguese',
    'da': 'Danish',
    'sv': 'Swedish',
    'no': 'Norwegian',
    'fi': 'Finnish',
    'is': 'Icelandic'
}

# Function to translate markdown content using Google Cloud
def translate_content(file_content, target_lang):
    translated = translate_client.translate(file_content, target_language=target_lang, source_language='en')
    return translated['translatedText']

# Iterate through all files in the source directory
for lang_code, lang_name in languages.items():
    print(f"Translating to {lang_name} ({lang_code})...")

    # Create the target language directory if it doesn't exist
    target_dir = f'docs/{lang_code}'
    os.makedirs(target_dir, exist_ok=True)

    for filename in os.listdir(source_dir):
        if filename.endswith('.md'):
            # Read the source markdown file
            with open(os.path.join(source_dir, filename), 'r', encoding='utf-8') as file:
                content = file.read()

            # Translate the content
            translated_content = translate_content(content, lang_code)

            # Write the translated content to the target directory
            with open(os.path.join(target_dir, filename), 'w', encoding='utf-8') as file:
                file.write(translated_content)

            print(f"Translated {filename} to {lang_name}.")

print("Translation process completed.")
