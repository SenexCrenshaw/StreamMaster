import { useChannelGroupsGetChannelGroupNamesQuery, useM3UFilesGetM3UFileNamesQuery, useProgrammesGetProgrammeNamesQuery } from "../../store/iptvApi";
import ChannelLogoEditor from "../ChannelLogoEditor";
import ChannelNameEditor from "../ChannelNameEditor";
import ChannelNumberEditor from "../ChannelNumberEditor";

import ChannelGroupEditor from "../channelGroups/ChannelGroupEditor";
import EPGEditor from "../epg/EPGEditor";
import createMultiSelectColumnConfigHook from "./createMultiSelectColumnConfigHook";

export const useChannelNumberColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_chno',
  EditorComponent: ChannelNumberEditor,
  headerTitle: 'Ch.',
  useFilter: false,
  width: 4
});

export const useChannelNameColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_name',
  EditorComponent: ChannelNameEditor,
  headerTitle: 'Name',
});

export const useEPGColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_ID',
  EditorComponent: EPGEditor,
  headerTitle: 'EPG',
  queryHook: useProgrammesGetProgrammeNamesQuery,
  width: 12
});

export const useChannelGroupColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_group',
  EditorComponent: ChannelGroupEditor,
  headerTitle: 'Group',
  queryHook: useChannelGroupsGetChannelGroupNamesQuery,
  width: 8
});

export const useM3UFileNameColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'm3UFileName',
  headerTitle: 'File',
  queryHook: useM3UFilesGetM3UFileNamesQuery,
  width: 8
});

export const useChannelLogoColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_logo',
  EditorComponent: ChannelLogoEditor,
  fieldType: 'image',
  headerTitle: 'Logo',
  useFilter: false
});
