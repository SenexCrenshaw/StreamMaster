import EPGEditor from '../epg/EPGEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelEPGColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'EPGId',
  EditorComponent: EPGEditor,
  headerTitle: 'EPG',
  maxWidth: 14,
  width: 14
});
