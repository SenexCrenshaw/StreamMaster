import { GetLineupPreviewChannel } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetLineupPreviewChannelRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetLineupPreviewChannel = createAsyncThunk('cache/getGetLineupPreviewChannel', async (param: GetLineupPreviewChannelRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetLineupPreviewChannel');
    const response = await GetLineupPreviewChannel(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});

