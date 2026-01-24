import { Environment } from '@abp/ng.core';

const baseUrl = 'http://linkvault.local';

const oAuthConfig = {
  issuer: 'http://linkvault.local/',
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
      url: 'http://linkvault.local',
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
