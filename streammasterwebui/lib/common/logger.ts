import log from 'loglevel';

log.setLevel(process.env.NODE_ENV === 'production' ? 'error' : 'debug');

function getCallerInfo() {
  const stack = new Error().stack;
  if (!stack) {
    return '';
  }

  const stackLines = stack.split('\n');
  const callerLine = stackLines[3] || '';
  const matchResult = callerLine.match(/\((.*):(\d+):(\d+)\)$/);

  if (matchResult && matchResult.length === 4) {
    const file = matchResult[1];
    const line = matchResult[2];
    const column = matchResult[3];
    return `${file}:${line}:${column}`;
  }
  return '';
}

const createLoggerFunction =
  (logFunction: (...args: any[]) => void) =>
  (message: string, ...args: any[]) => {
    const callerInfo = getCallerInfo();
    logFunction(`${message} (${callerInfo})`, ...args);
  };

export const Logger = {
  debug: createLoggerFunction(log.debug),
  error: createLoggerFunction(log.error),
  info: createLoggerFunction(log.info),
  warn: createLoggerFunction(log.warn)
};
