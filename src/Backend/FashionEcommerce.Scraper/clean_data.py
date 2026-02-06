import json
import os

# Define the placeholder URL to remove
PLACEHOLDER_URL = "https://dosi-in.com/images/assets/default-dosiin-product.png"

# Files to clean
files_to_clean = [
    r"d:\SUPER_PROJECT\FashionEcommerce\src\Backend\FashionEcommerce.API\Data\products_scraped.json",
    r"d:\SUPER_PROJECT\FashionEcommerce\src\Backend\FashionEcommerce.Scraper\products_scraped.json",
    r"d:\SUPER_PROJECT\FashionEcommerce\src\Backend\FashionEcommerce.Scraper\products_combined.json"
]

def clean_file(file_path):
    if not os.path.exists(file_path):
        print(f"⚠️ File not found: {file_path}")
        return

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        original_count = len(data)
        
        # Filter out products with the placeholder image
        cleaned_data = [
            product for product in data 
            if product.get('imageUrl') != PLACEHOLDER_URL
        ]
        
        new_count = len(cleaned_data)
        removed_count = original_count - new_count
        
        if removed_count > 0:
            with open(file_path, 'w', encoding='utf-8') as f:
                json.dump(cleaned_data, f, indent=2, ensure_ascii=False)
            print(f"✅ Cleaned {file_path}: Removed {removed_count} items (Remaining: {new_count})")
        else:
            print(f"✨ No placeholder images found in {file_path}")
            
    except Exception as e:
        print(f"❌ Error processing {file_path}: {e}")

if __name__ == "__main__":
    for file_path in files_to_clean:
        clean_file(file_path)
