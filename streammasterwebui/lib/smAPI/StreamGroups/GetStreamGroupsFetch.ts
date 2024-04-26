import { GetStreamGroups } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamGroups = createAsyncThunk('cache/getGetStreamGroups', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetStreamGroups');
    const response = await GetStreamGroups();
    console.log('Fetched GetStreamGroups ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


