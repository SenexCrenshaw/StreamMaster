import EPGEditor from '../epg/EPGEditor';
import createMultiSelectColumnConfigHook from './createVideoStreamMultiSelectColumnConfigHook';

export const useEPGColumnConfig = createMultiSelectColumnConfigHook({
  EditorComponent: EPGEditor,
  dataField: 'user_Tvg_ID',
  headerTitle: 'EPG',
  width: 12
});
