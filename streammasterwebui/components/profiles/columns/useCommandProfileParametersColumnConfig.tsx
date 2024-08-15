import { CommandProfileColumnConfigProps, useCommandProfileColumnConfig } from './useCommandProfileColumnConfig';

export const useCommandProfileParametersColumnConfig = (props?: CommandProfileColumnConfigProps) => {
  const test = useCommandProfileColumnConfig({ ...props, field: 'Parameters', header: 'Parameters' });
  return test;
};
