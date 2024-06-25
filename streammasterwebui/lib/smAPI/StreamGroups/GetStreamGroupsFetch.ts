import { GetStreamGroups } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetStreamGroups = createAsyncThunk('cache/getGetStreamGroups', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetStreamGroups');
    const response = await GetStreamGroups();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


