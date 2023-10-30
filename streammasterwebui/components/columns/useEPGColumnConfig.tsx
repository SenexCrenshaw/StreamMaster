import { useProgrammesGetProgrammeNamesQuery } from '@lib/iptvApi';
import EPGEditor from '../epg/EPGEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useEPGColumnConfig = createMultiSelectColumnConfigHook({
  EditorComponent: EPGEditor,
  dataField: 'user_Tvg_ID',
  headerTitle: 'EPG',
  queryHook: useProgrammesGetProgrammeNamesQuery,
  width: 12
});
