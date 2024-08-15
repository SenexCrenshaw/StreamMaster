import { CommandProfileColumnConfigProps, useCommandProfileColumnConfig } from './useCommandProfileColumnConfig';

export const useCommandProfileCommandColumnConfig = (props?: CommandProfileColumnConfigProps) => {
  const test = useCommandProfileColumnConfig({ ...props, field: 'Command', header: 'Command' });
  return test;
};
