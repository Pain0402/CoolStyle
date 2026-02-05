import asyncio
import json
import os
import re
from playwright.async_api import async_playwright

async def crawl_dosiin():
    print("🚀 Starting Specialized Dosi-in Scraper (v5)...")
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True, args=['--disable-blink-features=AutomationControlled'])
        context = await browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
            viewport={'width': 1920, 'height': 1080}
        )
        page = await context.new_page()
        
        url = "https://dosi-in.com/products?page=2&sort_by=random&sort_order=desc"
        try:
            await page.goto(url, wait_until="load", timeout=120000)
            await asyncio.sleep(5)
            
            for _ in range(15):
                await page.evaluate("window.scrollBy(0, 800)")
                await asyncio.sleep(1)
            
            products_data = await page.evaluate('''() => {
                const results = [];
                const cards = document.querySelectorAll('.product-item');
                cards.forEach(card => {
                    const brandEl = card.querySelector('.product-item_brand');
                    const nameEl = card.querySelector('.product-item_name');
                    const priceEl = card.querySelector('.product-item_price');
                    const imgEl = card.querySelector('img');
                    const linkEl = card.querySelector('a');
                    
                    if (priceEl && imgEl) {
                        const name = nameEl ? nameEl.innerText.trim() : card.innerText.split('\\n')[0].trim();
                        const brand = brandEl ? brandEl.innerText.trim() : "";
                        const src = imgEl.src || imgEl.getAttribute('data-src') || imgEl.getAttribute('lazy-src');
                        const link = linkEl ? linkEl.href : "";

                        results.push({ name, brand, priceText: priceEl.innerText, img: src, link });
                    }
                });
                return results;
            }''')

            await browser.close()
            
            cleaned = []
            seen_slugs = set()
            
            for item in products_data:
                brand_and_name = (item['brand'] + " " + item['name']).strip()
                if len(brand_and_name) < 5: continue
                
                # Slug logic
                raw_slug = ""
                if item['link'] and 'products' in item['link']:
                    raw_slug = item['link'].strip('/').split('/')[-1]
                
                if not raw_slug or len(raw_slug) < 3:
                    # Generate slug from name
                    raw_slug = re.sub(r'[^a-z0-9]+', '-', brand_and_name.lower()).strip('-')
                
                final_slug = "ds-" + raw_slug
                if final_slug in seen_slugs:
                    final_slug += "-" + str(len(seen_slugs))
                
                # Price logic
                nums = re.findall(r'\d+', item['priceText'].replace('.', '').replace(',', ''))
                if not nums: continue
                price = int(nums[-1])
                
                if item['img'] and item['img'].startswith('http'):
                    cleaned.append({
                        "name": brand_and_name,
                        "basePrice": price,
                        "imageUrl": item['img'],
                        "slug": final_slug
                    })
                    seen_slugs.add(final_slug)

            print(f"✅ Crawled {len(cleaned)} products.")
            with open("products_dosiin.json", "w", encoding="utf-8") as f:
                json.dump(cleaned, f, indent=2, ensure_ascii=False)
            
            # Combine with Coolmate
            coolmate_file = "products_scraped.json" # Original
            all_data = []
            if os.path.exists(coolmate_file):
                with open(coolmate_file, "r", encoding="utf-8") as f:
                    all_data = json.load(f)
            
            # Only add if name not exists
            names = {p['name'] for p in all_data}
            added = 0
            for p in cleaned:
                if p['name'] not in names:
                    all_data.append(p)
                    names.add(p['name'])
                    added += 1
            
            # Force uniqueness on slugs across all
            final_slugs = set()
            for p in all_data:
                base = p['slug']
                if base in final_slugs:
                    p['slug'] = base + "-" + str(len(final_slugs))
                final_slugs.add(p['slug'])

            with open("products_combined.json", "w", encoding="utf-8") as f:
                json.dump(all_data, f, indent=2, ensure_ascii=False)
            
            print(f"📂 Saved {len(all_data)} products to products_combined.json")
            
        except Exception as e:
            print(f"❌ Error: {e}")

if __name__ == "__main__":
    asyncio.run(crawl_dosiin())
