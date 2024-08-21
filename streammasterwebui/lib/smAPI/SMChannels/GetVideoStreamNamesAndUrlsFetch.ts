import { GetVideoStreamNamesAndUrls } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetVideoStreamNamesAndUrls = createAsyncThunk('cache/getGetVideoStreamNamesAndUrls', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetVideoStreamNamesAndUrls');
    const response = await GetVideoStreamNamesAndUrls();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


