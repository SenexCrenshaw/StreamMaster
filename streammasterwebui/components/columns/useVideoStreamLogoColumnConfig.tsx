import VideoStreamLogoEditor from '../videoStream/VideoStreamLogoEditor';
import createVideoStreamMultiSelectColumnConfigHook from './createVideoStreamMultiSelectColumnConfigHook';

export const useVideoStreamLogoColumnConfig = createVideoStreamMultiSelectColumnConfigHook({
  EditorComponent: VideoStreamLogoEditor,
  dataField: 'user_Tvg_logo',
  fieldType: 'image',
  headerTitle: '',
  useFilter: false
});
