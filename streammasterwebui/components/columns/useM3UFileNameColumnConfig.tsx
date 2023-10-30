import { useM3UFilesGetM3UFileNamesQuery } from '@lib/iptvApi';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useM3UFileNameColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'm3UFileName',
  headerTitle: 'File',
  maxWidth: 8,
  queryHook: useM3UFilesGetM3UFileNamesQuery
});
