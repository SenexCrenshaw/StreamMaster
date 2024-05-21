import log from 'loglevel';

log.setLevel(process.env.NODE_ENV === 'production' ? 'error' : 'debug');

export const Logger = {
  debug: (message: string, ...args: any[]) => log.debug(message, ...args),
  error: (message: string, ...args: any[]) => log.error(message, ...args),
  info: (message: string, ...args: any[]) => log.info(message, ...args),
  warn: (message: string, ...args: any[]) => log.warn(message, ...args)
};
