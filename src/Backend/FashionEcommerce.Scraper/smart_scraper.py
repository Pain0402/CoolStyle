import asyncio
import json
import os
from playwright.async_api import async_playwright

async def smart_crawl():
    print("🚀 Starting Smart Generic Scraper...")
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36"
        )
        page = await context.new_page()
        
        url = "https://www.coolmate.me/nam"
        print(f"🔗 Navigating to {url}...")
        await page.goto(url, wait_until="networkidle", timeout=60000)
        
        # Scroll
        print("📜 Scrolling...")
        for _ in range(3):
            await page.evaluate("window.scrollBy(0, 1500)")
            await asyncio.sleep(2)
            
        print("🔍 Attempting smart extraction...")
        products = await page.evaluate('''() => {
            const results = [];
            const seen = new Set();
            
            // 1. Find all price elements (containing 'đ' and numbers)
            const walkers = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT);
            let node;
            const priceNodes = [];
            while(node = walkers.nextNode()) {
                const text = node.textContent.trim();
                // Match patterns like "199.000đ" or "199.000 đ"
                if (text.includes('đ') && /\d/.test(text) && text.length < 20) {
                    priceNodes.push(node.parentElement);
                }
            }
            
            priceNodes.forEach(priceEl => {
                let current = priceEl;
                let foundImg = null;
                let foundName = null;
                
                // Climb up to find a container with an image and a title
                for (let i = 0; i < 8; i++) {
                    if (!current || current === document.body) break;
                    
                    if (!foundImg) foundImg = current.querySelector('img[src*="cdn"], img[src*="media"], img[src*="static"]');
                    if (!foundName) foundName = current.querySelector('h1, h2, h3, h4, h5, [class*="title"], [class*="name"]');
                    
                    if (foundImg && foundName && foundName.innerText.trim().length > 5) {
                        const name = foundName.innerText.trim();
                        if (!seen.has(name)) {
                            results.push({
                                name: name,
                                price: priceEl.innerText.trim(),
                                img: foundImg.src,
                                link: current.querySelector('a')?.href || ""
                            });
                            seen.add(name);
                        }
                        break;
                    }
                    current = current.parentElement;
                }
            });
            return results;
        }''')
        
        await browser.close()
        
        # Cleanup
        final_list = []
        for p in products:
            # Clean price
            p_text = p['price'].replace('.', '').replace('đ', '').replace(' ', '').replace(',', '').strip()
            try:
                # Sometimes it captures "299.000350.000" if sale price and old price are together
                # We take the first part
                import re
                prices = re.findall(r'\d+', p_text)
                price = int(prices[0]) if prices else 0
            except:
                price = 0
                
            if p['img'].startswith('http') and price > 0:
                final_list.append({
                    "name": p['name'],
                    "basePrice": price,
                    "imageUrl": p['img'],
                    "slug": p['link'].split('/')[-1] if p['link'] else p['name'].lower().replace(' ', '-')
                })
        
        print(f"✅ Extracted {len(final_list)} products.")
        
        with open("products_scraped.json", "w", encoding="utf-8") as f:
            json.dump(final_list, f, indent=2, ensure_ascii=False)
        print(f"📂 Saved to products_scraped.json")

if __name__ == "__main__":
    asyncio.run(smart_crawl())
