import asyncio
import json
import os
from playwright.async_api import async_playwright

async def crawl_coolmate():
    print("🚀 Starting Playwright Scraper for Coolmate...")
    async with async_playwright() as p:
        # Launch browser
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36"
        )
        page = await context.new_page()
        
        # Target URL
        url = "https://www.coolmate.me/nam"
        print(f"🔗 Navigating to {url}...")
        
        try:
            await page.goto(url, wait_until="networkidle", timeout=60000)
            
            # Scroll down to trigger lazy loading
            print("📜 Scrolling to load more products...")
            for i in range(5):
                await page.evaluate("window.scrollBy(0, 1500)")
                await asyncio.sleep(2)
            
            # Extract product data using JavaScript inside the page context
            print("🔍 Extracting product data...")
            products_data = await page.evaluate('''() => {
                const results = [];
                // Look for common product card selectors on Coolmate
                const cards = document.querySelectorAll('.product-card, .product-grid__item');
                
                cards.forEach(card => {
                    const nameEl = card.querySelector('.product-card__title, h3, .name');
                    const priceEl = card.querySelector('.product-card__price, .price, .current-price');
                    const imgEl = card.querySelector('img');
                    const linkEl = card.querySelector('a');
                    
                    if (nameEl && priceEl) {
                        results.append({
                            name: nameEl.innerText.trim(),
                            priceText: priceEl.innerText.trim(),
                            img: imgEl ? imgEl.src : "",
                            link: linkEl ? linkEl.href : ""
                        });
                    }
                });
                return results;
            }''')
            
            # Simple error fix: 'results.append' should be 'results.push' in JS
            # Let me rewrite the evaluate for safety
            products_data = await page.evaluate('''() => {
                const results = [];
                const cards = document.querySelectorAll('.product-card');
                cards.forEach(card => {
                    const name = card.querySelector('.product-card__title')?.innerText.trim();
                    const price = card.querySelector('.product-card__price')?.innerText.trim();
                    const img = card.querySelector('.product-card__image img')?.src;
                    const link = card.querySelector('a')?.href;
                    
                    if (name && price) {
                        results.push({ name, price, img, link });
                    }
                });
                return results;
            }''')

            await browser.close()
            
            # Data Cleaning & Formatting
            cleaned = []
            seen_names = set()
            
            for item in products_data:
                name = item['name']
                if name and name not in seen_names:
                    # Parse price: "199.000đ" -> 199000
                    raw_price = item['price'].replace('.', '').replace('đ', '').replace(' ', '').replace(',', '').strip()
                    try:
                        price = int(raw_price)
                    except:
                        price = 0
                        
                    cleaned.append({
                        "name": name,
                        "price": price,
                        "image_url": item['img'],
                        "slug": item['link'].split('/')[-1] if item['link'] else ""
                    })
                    seen_names.add(name)

            if not cleaned:
                print("⚠️ No products found. The selectors might have changed.")
                # Fallback to a broader search if first attempt failed
                return

            print(f"✅ Successfully crawled {len(cleaned)} products.")
            
            output_file = "products_coolmate.json"
            with open(output_file, "w", encoding="utf-8") as f:
                json.dump(cleaned, f, indent=2, ensure_ascii=False)
                
            print(f"📂 Data saved to: {os.path.abspath(output_file)}")
            
        except Exception as e:
            print(f"❌ An error occurred during crawling: {str(e)}")
            await browser.close()

if __name__ == "__main__":
    asyncio.run(crawl_coolmate())
