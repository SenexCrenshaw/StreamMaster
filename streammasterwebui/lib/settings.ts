// settings.ts

export const isClient = typeof window !== 'undefined';

export const isDev = process.env.NODE_ENV === 'development';

const loadConfig = () => {
  const request = new XMLHttpRequest();
  request.open('GET', '/config.json', false); // Synchronous request
  request.send(null);

  if (request.status === 200) {
    return JSON.parse(request.responseText);
  } else {
    throw new Error(`Failed to load config: ${request.status}`);
  }
};

const config = loadConfig();

export const defaultPort = config.defaultPort;

export const baseHostURL = isClient && !isDev ? `${window.location.protocol}//${window.location.host}` : `http://127.0.0.1:${defaultPort}`;
