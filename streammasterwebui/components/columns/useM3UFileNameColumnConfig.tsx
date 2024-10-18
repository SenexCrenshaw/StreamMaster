import createVideoStreamMultiSelectColumnConfigHook from './createVideoStreamMultiSelectColumnConfigHook';

export const useM3UFileNameColumnConfig = createVideoStreamMultiSelectColumnConfigHook({
  dataField: 'm3UFileName',
  headerTitle: 'File',
  width: 6,
  queryHook: useM3UFilesGetM3UFileNamesQuery
});
