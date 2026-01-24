import { Environment } from '@abp/ng.core';

const baseUrl = 'https://keyvaultx.netlify.app';

const oAuthConfig = {
  issuer: 'https://linkvault.runasp.net/',
  redirectUri: baseUrl,
  clientId: 'LinkVault_App',
  responseType: 'code',
  scope: 'offline_access LinkVault',
  requireHttps: false,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'LinkVault',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://linkvault.runasp.net',
      rootNamespace: 'LinkVault',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  // remoteEnv: {
  //   url: '/getEnvConfig',
  //   mergeStrategy: 'deepmerge'
  // }
} as Environment;
