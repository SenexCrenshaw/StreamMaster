import { useIntl } from 'react-intl';
import { toCamelCase } from './common';

export function GetMessage(...arguments_: string[]): string {
  const intl = useIntl();

  if (arguments_ === undefined || arguments_.length === 0 || arguments_[0] === '') {
    return '';
  }
  const ids: string[] = arguments_.flatMap((argument) => argument.split(' '));

  const message = ids.map((x) => intl.formatMessage({ id: x })).join(' ');

  if (message === toCamelCase(message)) {
    return arguments_.join('');
  }

  return message;
}

// export const GetMessageDiv = (id: string, upperCase?: boolean | null): React.ReactNode => {
//   const intl = useIntl();
//   const message = intl.formatMessage({ id });

//   if (upperCase) {
//     return <div>{message.toUpperCase()}</div>;
//   }

//   return <div>{message}</div>;
// };
