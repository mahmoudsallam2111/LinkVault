import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44317/',
  redirectUri: baseUrl,
  clientId: 'LinkVault_App',
  responseType: 'code',
  scope: 'offline_access LinkVault',
  requireHttps: true,
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
      url: 'https://localhost:44317',
      rootNamespace: 'LinkVault',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge',
  },
} as Environment;
