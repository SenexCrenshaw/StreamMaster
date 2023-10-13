export const isClient = typeof window !== 'undefined';

export const isDev = process.env.NODE_ENV === 'development';

export const baseHostURL = isClient && !isDev ? window.location.protocol + '//' + window.location.host : 'http://127.0.0.1:7095';
