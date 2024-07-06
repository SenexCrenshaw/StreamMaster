import { GetStreamGroupProfiles } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamGroupProfiles = createAsyncThunk('cache/getGetStreamGroupProfiles', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamGroupProfiles');
    const response = await GetStreamGroupProfiles();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


