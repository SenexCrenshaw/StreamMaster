import CommandProfileNameSelector from '../CommandProfileNameSelector';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMCommandProfileNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'CommandProfileName',
  EditorComponent: CommandProfileNameSelector,
  headerTitle: 'Profile Name',
  sortable: true,
  width: 125
});
