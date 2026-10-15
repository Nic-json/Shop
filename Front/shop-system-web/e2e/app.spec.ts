
import { test, expect } from '@playwright/test';

test.describe('ShopSys - Navegação', () => {
  test('deve abrir a aplicação', async ({ page }) => {
    await page.goto('/');

    await expect(page.locator('body')).toBeVisible();

    await expect(page).toHaveURL(
      'http://localhost:4200/'
    );
  });
});
