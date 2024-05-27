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

// Function to start a log group
const startGroup = (groupName: string) => {
  const callerInfo = getCallerInfo();
  console.group(`${groupName} (${callerInfo})`);
};

// Function to end a log group
const endGroup = () => {
  console.groupEnd();
};

// Helper function to log multiple messages within a group
const logGroup = (groupName: string, messages: { level: keyof typeof log; message: string; args?: any[] }[]) => {
  startGroup(groupName);
  messages.forEach(({ level, message, args }) => {
    if (typeof log[level] === 'function') {
      createLoggerFunction(log[level] as (...args: any[]) => void)(message, ...(args || []));
    }
  });
  endGroup();
};

export const Logger = {
  debug: createLoggerFunction(log.debug),
  endGroup,
  error: createLoggerFunction(log.error),
  info: createLoggerFunction(log.info),
  logGroup,
  startGroup,
  warn: createLoggerFunction(log.warn)
};
