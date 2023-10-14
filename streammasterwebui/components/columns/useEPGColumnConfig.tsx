import { useProgrammesGetProgrammeNamesQuery } from '@lib/iptvApi';
import EPGEditor from '../epg/EPGEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useEPGColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_ID',
  EditorComponent: EPGEditor,
  headerTitle: 'EPG',
  width: 12,
  queryHook: useProgrammesGetProgrammeNamesQuery,
});
