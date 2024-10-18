import EPGEditor from '../epg/EPGEditor';
import createMultiSelectColumnConfigHook from './createVideoStreamMultiSelectColumnConfigHook';

export const useEPGColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'EPGId',
  EditorComponent: EPGEditor,
  headerTitle: 'EPG',
  maxWidth: 4,
  width: 4
});
