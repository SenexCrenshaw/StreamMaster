import M3UFilesMaxStreamsEditor from '@components/m3u/M3UFilesMaxStreamsEditor';
import createM3UFileMultiSelectColumnConfigHook from './createM3UFileMultiSelectColumnConfigHook';

export const useM3UFilesMaxStreamsColumnConfig = createM3UFileMultiSelectColumnConfigHook({
  dataField: 'maxStreamCount',
  EditorComponent: M3UFilesMaxStreamsEditor,
  headerTitle: 'Max Streams',
  useFilter: false,
  width: 4
});
