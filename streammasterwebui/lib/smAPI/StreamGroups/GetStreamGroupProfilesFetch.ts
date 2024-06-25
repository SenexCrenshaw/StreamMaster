import { GetStreamGroupProfiles } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


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


