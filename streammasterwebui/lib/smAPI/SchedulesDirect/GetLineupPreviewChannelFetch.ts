import { GetLineupPreviewChannel } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetLineupPreviewChannelRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetLineupPreviewChannel = createAsyncThunk('cache/getGetLineupPreviewChannel', async (param: GetLineupPreviewChannelRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetLineupPreviewChannel');
    const response = await GetLineupPreviewChannel(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


