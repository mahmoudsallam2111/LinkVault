-- Update LinkVault_App to use the Netlify URL
UPDATE "OpenIddictApplications"
SET 
    "RedirectUris" = '["https://keyvaultx.netlify.app"]', 
    "PostLogoutRedirectUris" = '["https://keyvaultx.netlify.app"]', 
    "ClientUri" = 'https://keyvaultx.netlify.app'
WHERE "ClientId" = 'LinkVault_App';

-- Update LinkVault_Swagger to use the HTTPS backend URL
UPDATE "OpenIddictApplications"
SET 
    "RedirectUris" = '["https://linkvault.runasp.net/swagger/oauth2-redirect.html"]', 
    "ClientUri" = 'https://linkvault.runasp.net/swagger'
WHERE "ClientId" = 'LinkVault_Swagger';
