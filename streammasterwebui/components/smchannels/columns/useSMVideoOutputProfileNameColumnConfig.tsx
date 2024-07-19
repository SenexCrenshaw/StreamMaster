import VideoOutputProfileNameSelector from '../VideoOutputProfileNameSelector';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMVideoOutputProfileNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'VideoOutputProfileName',
  EditorComponent: VideoOutputProfileNameSelector,
  headerTitle: 'Profile Name',
  sortable: true,
  width: 125
});
